rm ../docs -d -r
ng build --prod --base-href "https://sharpiro.github.io/CryptoSharp/"
cp dist/index.html dist/404.html
cp dist ../docs -r