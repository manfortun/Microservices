## Browse Bay
This is an e-commerce web application that allows users to browse, search for, and purchase products. Users can create accounts, log in, etc. In this application, there are 3 types of users

1. Buyer - this type of user can purchase items
2. Seller - sellers can create their own products for listing in the market, and also buy products in the market
3. Admin - admins can add/modify/remove categories. Admins can also do what sellers and buyers do.

## Microservices
This system contains 2 services:

1. *AuthService* - responsible for handling account-related requests such as logging in, change password, authentication
2. *CatalogService* - handles all requests related to products, categories, and purchases.

## Deployment
##### Download the "K8S" folder and run the following commands:
`kubectl apply -f authservice-depl.yaml`

`kubectl apply -f catalogservice-depl.yaml`

`kubectl apply -f ingress-srv.yaml`

`kubectl apply -f local-pvc.yaml`

`kubectl apply -f mssql-depl.yaml`

`kubectl apply -f rabbitmq-depl.yaml`

##### Download BrowseBay folder and run.
