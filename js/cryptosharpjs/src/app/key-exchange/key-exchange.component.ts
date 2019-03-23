import { Component, OnInit } from '@angular/core'
import * as crypto from "crypto-browserify"
import { CustomFormControl, hexValidator } from '../shared/custom-form-control'
import { Validators } from '@angular/forms'
import { Buffer } from 'buffer'
import * as toastr from "toastr"
import { flashAnimation, flashState } from '../shared/custom-animations'

@Component({
  selector: 'app-key-exchange',
  templateUrl: './key-exchange.component.html',
  styleUrls: ['./key-exchange.component.css'],
  animations: [flashAnimation]
})
export class KeyExchangeComponent implements OnInit {
  myPubKeyControl = new CustomFormControl()
  otherPubKeyControl = new CustomFormControl('', [Validators.required, hexValidator, Validators.minLength(65 * 2), Validators.maxLength(65 * 2)])
  secretControl = new CustomFormControl()
  myPubKeyControlState: flashState = ''
  otherPubKeyControlState: flashState = ''
  secretControlState: flashState = ''
  mySecrets: crypto.ECDH

  constructor() { }

  ngOnInit() { }

  onGenerateKeysClick() {
    this.mySecrets = crypto.createECDH('secp256k1')
    const myPubKey = this.mySecrets.generateKeys()
    this.myPubKeyControl.setValue(myPubKey.toString('hex'))
    this.myPubKeyControlState = "greenflash"
  }

  onGenerateSecretClick() {
    if (!this.mySecrets) {
      toastr.error("Keys must be generated")
      return
    }
    if (this.otherPubKeyControl.invalidOrEmpty) {
      this.otherPubKeyControlState = "redflash"
      this.otherPubKeyControl.markAsDirty()
      return
    }
    const otherPubKeyBuffer = Buffer.from(this.otherPubKeyControl.value, "hex")
    const mySecret = this.mySecrets.computeSecret(otherPubKeyBuffer)
    this.secretControl.setValue(mySecret.toString("hex"))
    this.secretControlState = "greenflash"
  }
}