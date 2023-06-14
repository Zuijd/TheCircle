var keys = createKeyPair();
var digSig;

function createKeyPair() {
    const keyPair = forge.pki.rsa.generateKeyPair({ bits: 2048 });

    const publicKey = forge.pki.publicKeyToPem(keyPair.publicKey);
    const privateKey = forge.pki.privateKeyToPem(keyPair.privateKey);

    return {
        publicKey: publicKey,
        privateKey: privateKey
    };
}

function createDigSig(message, privateKey) {
    const privateKeyObj = forge.pki.privateKeyFromPem(privateKey);

    const data = forge.util.encodeUtf8(message);

    const md = forge.md.sha256.create();
    md.update(data);
    const signature = privateKeyObj.sign(md);

    digSig = signature;
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


