# Kubernetes cluster configuration
These are configuration instructions for an Azure AKS cluster.

## Prerequisites
This assumes you are familiar with and have installed Helm.

## Configuration
```
kubectl create namespace payment-processor-demo
kubectl apply -f .\setup\helm-service-account.yaml
helm init --service-account tiller

az network public-ip create \
    -g MC_kubernetes_rp-aks-centralus_centralus \
    -n rp-aks-centralus-payment-processor-demo-ingress-ip \
    -l centralus \
    --allocation-method static \
    --dns-name rp-aks-centralus-payment-processor-demo-ingress

helm install stable/nginx-ingress \
    --name nginx-ingress \
    --namespace api-ingress \
    --set controller.service.loadBalancerIP=ip.address.created.above \
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