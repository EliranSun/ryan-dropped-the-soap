# dont-drop-the-soap
 
## Butler Version Push

### install butler
```
curl -L https://broth.itch.ovh/butler/darwin-amd64/LATEST/archive/default -o butler.zip
unzip butler.zip
chmod +x butler
mv butler /usr/local/bin
```

### login to butler
```
butler login
```

### push to prod
```
butler push 23.07.2024 ryanmcklain/ryan-dropped-the-soap:html --userversion 0.5.2
```