// https://honest.cash/cpacia/understanding-schnorr-signatures-3247/

const EC = require('elliptic').ec;
const BN = require('bn.js');
const crypto = require('crypto');

const curve = new EC('secp256k1')
const generatorPoint = curve.g
const message = "my message to sign"
const persistentPublicKey = curve.keyFromPublic(Buffer.from("034bf1b5e7104411fd81c7f41dd3f4c424d054abedd012bfa4aacd47658cd15892", "hex")).getPublic()

const { ephemeralPublicKey, signature } = createSignature()
const actualEphemeralPublicKey = verifySignature()

console.log(ephemeralPublicKey.encode("hex"))
console.log(actualEphemeralPublicKey.encode("hex"))
console.log(ephemeralPublicKey.encode("hex") === actualEphemeralPublicKey.encode("hex"))

function createSignature() {
    const persistentPrivateKey = new BN("c071d7cb8accebb9c6f00e133fe3b8834fd52c0b25e9087f0e0c266b892addd0")
    const ephemeralPrivateKey = new BN(crypto.randomBytes(32))
    const ephemeralPublicKey = generatorPoint.mul(ephemeralPrivateKey)

    const hashNumber = getHashNumber(ephemeralPublicKey, persistentPublicKey)
    const signature = hashNumber.mul(persistentPrivateKey).add(ephemeralPrivateKey)

    return { ephemeralPublicKey, signature }
}

function verifySignature() {
    const hashNumber = getHashNumber(ephemeralPublicKey, persistentPublicKey)
    const actualEphemeralPublicKey = (generatorPoint.mul(signature)).add((persistentPublicKey.mul(hashNumber).neg()))
    return actualEphemeralPublicKey
}

function getHashNumber(ephemeralPublicKey, persistentPublicKey) {
    const ephemeralPublicKeyBuffer = Buffer.from(ephemeralPublicKey.encode("array"))
    const persistentPublicKeyBuffer = Buffer.from(persistentPublicKey.encode("array"))
    const messageBuffer = Buffer.from(message)
    const buffer = Buffer.concat([ephemeralPublicKeyBuffer, persistentPublicKeyBuffer, messageBuffer])
    const hashAlg = crypto.createHash("sha256")
    hashAlg.update(buffer)
    const hashHex = hashAlg.digest().toString("hex")
    const hashNumber = new BN(hashHex)
    return hashNumber
}