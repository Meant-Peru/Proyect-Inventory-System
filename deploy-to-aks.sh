#!/bin/bash
set -e

# Script de deployment manual a AKS
# Uso: ./deploy-to-aks.sh [dev|qa|prd]

ENVIRONMENT=$1
GRUPO="${GRUPO:-AppG5}"

if [ -z "$ENVIRONMENT" ]; then
    echo "❌ Error: Debes especificar un ambiente (dev, qa, prd)"
    echo "Uso: ./deploy-to-aks.sh [dev|qa|prd]"
    exit 1
fi

if [[ ! "$ENVIRONMENT" =~ ^(dev|qa|prd)$ ]]; then
    echo "❌ Error: Ambiente inválido. Usa: dev, qa, o prd"
    exit 1
fi

echo "🚀 Iniciando deployment a AKS"
echo "   Ambiente: $ENVIRONMENT"
echo "   Grupo: $GRUPO"
echo ""

# Configurar variables según el ambiente
case $ENVIRONMENT in
    dev)
        RESOURCE_GROUP="rg-appg5-dev-west-01-${GRUPO}"
        AKS_NAME="aks-appg5-dev-west-01-${GRUPO}"
        REPLICAS=1
        ;;
    qa)
        RESOURCE_GROUP="rg-appg5-qa-west-01-${GRUPO}"
        AKS_NAME="aks-appg5-qa-west-01-${GRUPO}"
        REPLICAS=2
        ;;
    prd)
        RESOURCE_GROUP="rg-appg5-prd-west-01-${GRUPO}"
        AKS_NAME="aks-appg5-prd-west-01-${GRUPO}"
        REPLICAS=3
        ;;
esac

# Verificar herramientas
if ! command -v kubectl &> /dev/null; then
    echo "❌ Error: kubectl no está instalado"
    exit 1
fi

if ! command -v az &> /dev/null; then
    echo "❌ Error: Azure CLI no está instalado"
    exit 1
fi

# Verificar login en Azure
echo "🔐 Verificando sesión de Azure..."
if ! az account show &> /dev/null; then
    echo "❌ No estás autenticado en Azure. Ejecuta: az login"
    exit 1
fi

# Obtener credenciales de AKS
echo "🔑 Obteniendo credenciales de AKS..."
az aks get-credentials --resource-group "$RESOURCE_GROUP" --name "$AKS_NAME" --overwrite-existing

# Verificar conexión
echo "✅ Verificando conexión al cluster..."
kubectl cluster-info

# Crear namespace si no existe
echo "📦 Creando namespace..."
kubectl get namespace inventory-system &> /dev/null || kubectl create namespace inventory-system

# Solicitar SQL Connection String si no está configurado
if [ -z "$SQL_CONNECTION_STRING" ]; then
    echo ""
    echo "⚠️  Se requiere SQL Connection String"
    echo "Puedes obtenerlo de Terraform con: cd ../Proyecto-Infra-G5 && terraform output sql_connection_string"
    echo ""
    read -p "Ingresa SQL Connection String: " SQL_CONNECTION_STRING
fi

# Crear/Actualizar Secret
echo "🔐 Creando/Actualizando secret de SQL..."
kubectl create secret generic sql-connection-secret \
    --from-literal=connection-string="$SQL_CONNECTION_STRING" \
    --namespace=inventory-system \
    --dry-run=client -o yaml | kubectl apply -f -

# Crear/Actualizar ConfigMap
echo "📝 Creando/Actualizando ConfigMap..."
kubectl create configmap inventory-config \
    --from-literal=ENV="$ENVIRONMENT" \
    --from-literal=API_PROVIDER_URL="${API_PROVIDER_URL:-}" \
    --namespace=inventory-system \
    --dry-run=client -o yaml | kubectl apply -f -

# Solicitar tag de imagen
ACR_NAME="acr${GRUPO}"
IMAGE_NAME="${ACR_NAME}.azurecr.io/inventory-system-app"

echo ""
read -p "Ingresa el tag de la imagen [latest]: " IMAGE_TAG
IMAGE_TAG=${IMAGE_TAG:-latest}

FULL_IMAGE="$IMAGE_NAME:$IMAGE_TAG"
echo "📦 Usando imagen: $FULL_IMAGE"

# Actualizar deployment.yaml con la imagen
echo "🔧 Actualizando deployment con la imagen..."
cp k8s/base/deployment.yaml k8s/base/deployment.yaml.bak
sed "s|IMAGE_PLACEHOLDER|$FULL_IMAGE|g" k8s/base/deployment.yaml.bak > k8s/base/deployment.yaml

# Aplicar manifiestos
echo "🚀 Desplegando aplicación..."
kubectl apply -k k8s/base

# Restaurar deployment.yaml
mv k8s/base/deployment.yaml.bak k8s/base/deployment.yaml

# Esperar a que el deployment esté listo
echo "⏳ Esperando a que el deployment esté listo..."
kubectl rollout status deployment/inventory-system-app -n inventory-system --timeout=5m

# Obtener IP del servicio
echo "🌐 Obteniendo IP externa del servicio..."
echo "   (Esto puede tomar varios minutos...)"

for i in {1..60}; do
    EXTERNAL_IP=$(kubectl get service inventory-system-service -n inventory-system -o jsonpath='{.status.loadBalancer.ingress[0].ip}' 2>/dev/null || echo "")

    if [ -n "$EXTERNAL_IP" ]; then
        echo ""
        echo "✅ Deployment completado exitosamente!"
        echo ""
        echo "🌐 Aplicación disponible en:"
        echo "   http://$EXTERNAL_IP"
        echo ""
        echo "📊 Estado del deployment:"
        kubectl get pods -n inventory-system
        echo ""
        echo "🔍 Para ver logs:"
        echo "   kubectl logs -n inventory-system deployment/inventory-system-app"
        exit 0
    fi

    echo -n "."
    sleep 5
done

echo ""
echo "⚠️  No se pudo obtener la IP externa automáticamente"
echo "Verifica manualmente con:"
echo "   kubectl get service inventory-system-service -n inventory-system"
