//function createDigSig(message, privateKey) {
//    // Maak een nieuwe instantie van JSEncrypt
//    var encrypt = new JSEncrypt();

//    // Set de private key als Base64-string
//    encrypt.setPrivateKey(atob(privateKey));

//    // Encrypteer het bericht met de private key
//    var encryptedMessage = encrypt.encrypt(message);

//    // 
//    document.getElementById("signature").value = encryptedMessage;

//    // 
//    document.getElementById("submit").submit();
//}


//function verifyDigSig(message, signature, publicKey) {
//    //create public key object from PEM-encoded key
//    const publicKeyObj = forge.pki.publicKeyFromPem(publicKey);

//    //convert message into bytes
//    const messageBytes = forge.util.encodeUtf8(message);

//    //hash the message using sha256 hashing algorithm
//    const md = forge.md.sha256.create();
//    md.update(messageBytes);   

//    //check the integrity of the send message
//    publicKeyObj.verify(md.digest().bytes(), signature) ? alert("GOED BERICHT") : alert("KUT BERICHT");
//}
