//var keys = createKeyPair();
var digSig;

//function createKeyPair() {
//    const keyPair = forge.pki.rsa.generateKeyPair({ bits: 2048 });
//
//    const publicKey = forge.pki.publicKeyToPem(keyPair.publicKey);
//    const privateKey = forge.pki.privateKeyToPem(keyPair.privateKey);
//
//    return {
//        publicKey: publicKey,
//        privateKey: privateKey
//    };
//}

async function importPrivateKey(privateKeyBytes) {
    try {
        // Converteer de byte-array naar een ArrayBuffer
        const privateKeyBuffer = new Uint8Array(privateKeyBytes).buffer;

        // Importeer de privésleutel met behulp van de Web Crypto API
        const privateKey = await crypto.subtle.importKey(
            'pkcs8',
            privateKeyBuffer,
            { name: 'RSA-OAEP', hash: 'SHA-256' },
            false,
            ['decrypt']
        );
        alert(privateKey)

        // Gebruik de privateKey zoals gewenst
        return privateKey;

    } catch (error) {
        // Behandel eventuele fouten die kunnen optreden
        console.error(error);
    }
}

function createDigSig(message, privateKey) {
    //
    const privateKeyRSA = importPrivateKey(privateKey)

    // 
    const privateKeyObj = forge.pki.privateKeyFromPem(privateKeyRSA)

    // 
    const data = forge.util.encodeUtf8(message);

    // 
    const md = forge.md.sha256.create();
    md.update(data);
    const signature = privateKeyObj.sign(md);

    // 
    const signatureHex = forge.util.bytesToHex(signature);

    alert(signatureHex);

    // 
    document.getElementById("signature").value = "HOI";

    // 
    document.getElementById("submit").submit();
}


function verifyDigSig(message, signature, publicKey) {
    //create public key object from PEM-encoded key
    const publicKeyObj = forge.pki.publicKeyFromPem(publicKey);

    //convert message into bytes
    const messageBytes = forge.util.encodeUtf8(message);

    //hash the message using sha256 hashing algorithm
    const md = forge.md.sha256.create();
    md.update(messageBytes);   

    //check the integrity of the send message
    publicKeyObj.verify(md.digest().bytes(), signature) ? alert("GOED BERICHT") : alert("KUT BERICHT");
}


