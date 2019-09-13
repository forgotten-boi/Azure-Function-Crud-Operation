echo "Updating variables"

echo $REDIRECT_URL

# export REDIRECT_URL=https://google.com
sed "s@http://localhost:3000@$REDIRECT_URL@g" ./src/config/adal.example.js > ./src/config/adal.js

echo "Updated file\n\n"
tail ./src/config/adal.js
