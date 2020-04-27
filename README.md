# TP4 Patrons Modeles

Ce TP se compose de trois services et une application principale

## Utilisation

Pour pouvoir utiliser ce système, il est nécessaire que RabbitMQ soit installé et lancé sur votre machine. 

### Lancer le service de gestion des utilisateurs

```sh
cd UserManager/
dotnet run
```
Ce service va charger une liste d’utilisateur depuis un fichier JSON contenant un tableau d’utilisateur puis retourner l’utilisateur correspondant à un username envoyé en requête

### Lancer le service de gestion du stock

```sh
cd StockManager/
dotnet run
```
Ce service va charger un stock depuis un fichier JSON contenant un tableau de produits puis diminuer ou augmenter les stocks en fonction des demandes des utilisateurs.

### Lancer le service de facturation

```sh
cd BillManager/
dotnet run
```
Ce service va juste retourner les différents totaux à notre application principale en basant sur le panier de l'utilisateur.

### Lancer l'application principale 

```sh
cd AppECommerce/
dotnet run
```
Cette application demande le username de l’utilisateur pour l'authentifier puis elle lui permet d’ajouter des items du stock dans un panier pour enfin générer la facture de ses items lorsqu'il a fini



