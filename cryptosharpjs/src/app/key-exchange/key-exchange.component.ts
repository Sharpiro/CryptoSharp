import { Component, OnInit } from '@angular/core';
import * as cryptoBrowserify from "crypto-browserify"
import * as cryptoType from "crypto"
import { CustomFormControl, hexValidator } from '../shared/custom-form-control';
import { Validators } from '@angular/forms';

const crypto: typeof cryptoType = cryptoBrowserify

@Component({
  selector: 'app-key-exchange',
  templateUrl: './key-exchange.component.html',
  styleUrls: ['./key-exchange.component.css']
})
export class KeyExchangeComponent implements OnInit {
  myPubKeyControl = new CustomFormControl()
  otherPubKeyControl = new CustomFormControl('', [hexValidator, Validators.minLength(65 * 2), Validators.maxLength(65 * 2)])
  secretControl = new CustomFormControl()
  mySecrets: cryptoType.ECDH

  constructor() { }

  ngOnInit() { }

  onGenerateKeysClick() {
    this.mySecrets = crypto.createECDH('secp256k1');
    const myPubKey = this.mySecrets.generateKeys();
    this.myPubKeyControl.setValue(myPubKey.toString('hex'))
    // console.log(myPubKey.toString('hex'));
  }

  onGenerateSecretClick() {
    if (!this.mySecrets) {
      console.error("secrets must be generated")
      return
    }
    if (this.otherPubKeyControl.invalidOrEmpty) {
      console.error("invalid 'other' public key")
      return
    }
    const otherPubKeyBuffer = Buffer.from(this.otherPubKeyControl.value, "hex")
    const mySecret = this.mySecrets.computeSecret(otherPubKeyBuffer);
    this.secretControl.setValue(mySecret.toString("hex"))
    // console.log(mySecret.toString("hex"));
  }
}