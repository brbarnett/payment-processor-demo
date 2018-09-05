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
kubectl apply -f ./setup/helm-service-account.yaml
helm init --service-account tiller

az network public-ip create \
    -g MC_kubernetes_rp-aks-centralus_centralus \
    -n rp-aks-centralus-payment-processor-demo-ingress-ip \
    -l centralus \
    --allocation-method static \
    --dns-name rp-aks-centralus-payment-processor-demo-ingress

helm install stable/nginx-ingress \
    --name nginx-ingress \
    --namespace payment-processor-demo \
    --set controller.service.loadBalancerIP=104.43.215.139 \
    --set controller.scope.enabled=true \
    --set controller.scope.namespace="payment-processor-demo" \
    --set controller.replicaCount=3

helm install stable/cert-manager \
    --name cert-manager \
    --namespace kube-system \
    --set ingressShim.defaultIssuerName=letsencrypt-prod `
    --set ingressShim.defaultIssuerKind=Issuer

kubectl apply -f .\setup\cert-issuer-prod.yaml -n payment-processor-demo

kubectl apply -f . -n payment-processor-demo
kubectl get services -n payment-processor-demo
kubectl get deployments -n payment-processor-demo
kubectl get pods -n payment-processor-demo
```