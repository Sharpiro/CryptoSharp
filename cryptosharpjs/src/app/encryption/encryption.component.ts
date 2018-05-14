import { Component, OnInit } from '@angular/core';
import { CustomFormControl, hexValidator } from '../shared/custom-form-control';
import { Validators } from '@angular/forms';
import * as cryptoBrowserify from "crypto-browserify"
import * as cryptoType from "crypto"
import { Buffer } from "buffer"

const crypto: typeof cryptoType = cryptoBrowserify

@Component({
  selector: 'app-encryption',
  templateUrl: './encryption.component.html',
  styleUrls: ['./encryption.component.css']
})
export class EncryptionComponent implements OnInit {
  keyControl = new CustomFormControl('', [Validators.minLength(32 * 2), Validators.maxLength(32 * 2), hexValidator])
  ivControl = new CustomFormControl('', [Validators.minLength(16 * 2), Validators.maxLength(16 * 2), hexValidator])
  plainControl = new CustomFormControl('', [hexValidator])
  cryptoControl = new CustomFormControl()
  // cryptoControl = new CustomFormControl('', [Validators.minLength(16 * 2), hexValidator])
  autoIV = true

  constructor() {
    this.plainControl.setValue("aaffaa")
  }

  ngOnInit() { }

  onGenerateClick() {
    this.keyControl.setValue(crypto.randomBytes(32).toString("hex"))
    this.ivControl.setValue(crypto.randomBytes(16).toString("hex"))
  }

  onEncryptClick() {
    if (this.keyControl.invalidOrEmpty || this.plainControl.invalidOrEmpty) {
      console.log("failure")
      return
    }
    if (!this.autoIV && this.ivControl.invalidOrEmpty) {
      console.log("Must provide an IV when 'autoIV' is disabled")
      return
    }
    const key = Buffer.from(this.keyControl.value, "hex")
    const iv = this.autoIV ? crypto.randomBytes(16) : Buffer.from(this.ivControl.value, "hex")
    const cipher = crypto.createCipheriv('aes-256-cbc', key, iv)

    let encrypted = cipher.update(this.plainControl.value, 'utf8')

    encrypted = Buffer.concat([encrypted, cipher.final()])
    if (this.autoIV) {
      encrypted = Buffer.concat([encrypted, iv])
    }
    // console.log(encrypted.toString("hex"))
    // console.log(iv.toString("hex"));
    this.cryptoControl.setValue(encrypted.toString("hex"))
  }

  onDecryptClick() {
    if (this.keyControl.invalidOrEmpty || this.cryptoControl.invalidOrEmpty) {
      console.log("failure")
      return
    }
    if (!this.autoIV && this.ivControl.invalidOrEmpty) {
      console.log("Must provide an IV when 'autoIV' is disabled")
      return
    }
    const key = Buffer.from(this.keyControl.value, "hex")

    let iv: Buffer
    let encryptedBuffer = Buffer.from(this.cryptoControl.value, "hex")
    if (this.autoIV) {
      iv = encryptedBuffer.slice(-16)
      encryptedBuffer = encryptedBuffer.slice(0, -16)
    } else {
      iv = Buffer.from(this.ivControl.value, "hex")
    }

    const decipher = crypto.createDecipheriv('aes-256-cbc', key, iv)
    let decrypted = decipher.update(encryptedBuffer)
    decrypted = Buffer.concat([decrypted, decipher.final()])
    console.log(decrypted.toString("utf8"))
    this.plainControl.setValue(decrypted.toString("utf8"))
  }
}