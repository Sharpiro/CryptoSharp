// https://honest.cash/cpacia/understanding-schnorr-signatures-3247/
// http://blog.oleganza.com/post/162861219668/eli5-how-digital-signatures-actually-work

const EC = require('elliptic').ec;
const BN = require('bn.js');
const crypto = require('crypto');

const curve = new EC('secp256k1')
const message = "my message to sign"
const persistentPrivateKey = new BN("c071d7cb8accebb9c6f00e133fe3b8834fd52c0b25e9087f0e0c266b892addd0")
const persistentPublicKey = curve.keyFromPublic(Buffer.from("034bf1b5e7104411fd81c7f41dd3f4c424d054abedd012bfa4aacd47658cd15892", "hex")).getPublic()

const { commitment_R, blindedSecret } = createSignature(persistentPrivateKey, message)
const signatureResult = verifySignature(persistentPublicKey, commitment_R, blindedSecret, message)

console.log(commitment_R.encode("hex"))
console.log(signatureResult)

function createSignature(persistentPrivateKey, message) {
    const nonce_r = new BN(crypto.randomBytes(32))
    const commitment_R = curve.g.mul(nonce_r)
    const hashE = getHashNumber(commitment_R, message)
    const blindedSecret = hashE.mul(persistentPrivateKey).add(nonce_r)
    return { commitment_R, blindedSecret }
}

function verifySignature(persistentPublicKey, commitment_R, blindedSecret, message) {
    const hashE = getHashNumber(commitment_R, message)
    const expectedSignature = curve.g.mul(blindedSecret);
    const actualSignature = persistentPublicKey.mul(hashE).add(commitment_R)

    // optionally calculate R commitment value and check it matches
    const actualCommitment_R = (expectedSignature).add((persistentPublicKey.mul(hashE).neg()))
    if (actualCommitment_R.encode("hex") !== commitment_R.encode("hex")) throw new Error("R value compute mismatch")

    return expectedSignature.encode("hex") === actualSignature.encode("hex")
}

function getHashNumber(ephemeralPublicKey, message) {
    const ephemeralPublicKeyBuffer = Buffer.from(ephemeralPublicKey.encode("array"))
    const messageBuffer = Buffer.from(message)
    const buffer = Buffer.concat([ephemeralPublicKeyBuffer, messageBuffer])
    const hashHex = crypto.createHash("sha256").update(buffer).digest().toString("hex")
    const hashNumber = new BN(hashHex)
    return hashNumber
}
