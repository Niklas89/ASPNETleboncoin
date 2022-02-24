ajout de la string de connexion à la base de données
pour utiliser dotnet-secrets il faut se connecter avec un compte sql server
suivre ce truc pour activer le compte sa et mettre un mdp 
https://www.milesweb.in/hosting-faqs/enable-sa-user-sql-server2019/
` dotnet user-secrets set  "dbString" "Server=<URL>;Database=<NAME>;User ID=<USERNAME>;Password=<PASSWORD>"`