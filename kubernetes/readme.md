# Kubernetes cluster configuration
These are configuration instructions for an Azure AKS cluster.

## Prerequisites
This assumes you are familiar with and have installed Helm.

## Configuration
```
az aks create \
    -g Kubernetes \
    -n rp-aks-centralus \
    -l centralus \
    -c 3 \
    -s Standard_DS1_v2 \
    --aad-server-app-id 00000000-0000-0000-0000-000000000000 \
    --aad-server-app-secret xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx= \
    --aad-client-app-id 00000000-0000-0000-0000-000000000000 \
    --aad-tenant-id 00000000-0000-0000-0000-000000000000 \
    --kubernetes-version 1.11.2 \
    --no-wait

kubectl create namespace payment-processor-demo
kubectl apply -f ./kubernetes/setup/helm-service-account.yaml
helm init --service-account tiller

az network public-ip create \
    -g MC_kubernetes_rp-aks-centralus_centralus \
    -n rp-aks-centralus-ppdi-ip \
    -l centralus \
    --allocation-method static \
    --dns-name rp-aks-centralus-ppdi

# Clone https://github.com/istio/istio and checkout tag 1.0.2. Run this from the cloned Istio directory
helm install install/kubernetes/helm/istio \
    --name istio \
    --namespace istio-system \
    --set global.mtls.enabled=true \
    --set grafana.enabled=true \
    --set grafana.persist=true \
    --set servicegraph.enabled=true \
    --set tracing.enabled=true \
    --set kiali.enabled=true

helm install stable/nginx-ingress \
    --name nginx-ingress \
    --namespace payment-processor-demo \
    --set controller.service.loadBalancerIP=23.99.176.141 \
    --set controller.scope.enabled=true \
    --set controller.scope.namespace="payment-processor-demo" \
    --set controller.replicaCount=3

helm install stable/cert-manager \
    --name cert-manager \
    --namespace kube-system \
    --set ingressShim.defaultIssuerName=letsencrypt-prod \
    --set ingressShim.defaultIssuerKind=Issuer

kubectl apply -f ./kubernetes/setup/cert-issuer-prod.yaml -n payment-processor-demo

helm install stable/rabbitmq \
    --name message-broker \
    --namespace payment-processor-demo \
    --set replicas=3 \
    --set persistence.enabled=true \
    --set rabbitmq.username=rabbitmq \
    --set rabbitmq.password=rabbitmq \
    --set rabbitmq.erlangCookie=SWQOKODSQALRPCLNMEQG

helm install stable/redis \
    --name payment-processor-db \
    --namespace payment-processor-demo \
    --set usePassword=false \
    --set cluster.slaveCount=3 \
    --set rbac.create=true

kubectl apply -f ./kubernetes -n payment-processor-demo

kubectl get services -n payment-processor-demo
kubectl get deployments -n payment-processor-demo
kubectl get pods -n payment-processor-demo
```