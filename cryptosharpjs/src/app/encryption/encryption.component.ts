import { Component, OnInit } from '@angular/core'
import { CustomFormControl, hexValidator, base64Validator } from '../shared/custom-form-control'
import { Validators } from '@angular/forms'
import * as crypto from "crypto-browserify"
import { Buffer } from "buffer"
import * as toastr from "toastr"
import { trigger, state, transition, animate, style, keyframes } from '@angular/animations'
import { flashAnimation, flashState } from '../shared/custom-animations'
import { FileInfo } from '../shared/fileInfo';
import { downloadFile } from '../shared/extensions';
import { encoding } from '../shared/types/custom-types';

@Component({
  selector: 'app-encryption',
  templateUrl: './encryption.component.html',
  styleUrls: ['./encryption.component.css'],
  animations: [flashAnimation]
})
export class EncryptionComponent implements OnInit {
  keyControl = new CustomFormControl('', [Validators.required, Validators.minLength(32 * 2), Validators.maxLength(32 * 2), hexValidator])
  ivControl = new CustomFormControl('', [Validators.required, Validators.minLength(16 * 2), Validators.maxLength(16 * 2), hexValidator])
  plainControl = new CustomFormControl('', Validators.required)
  cryptoControl = new CustomFormControl('', [Validators.required, hexValidator, Validators.minLength(16 * 2)])
  autoIV = true
  encoding: encoding = "utf8"
  keyControlState: flashState = ''
  ivControlState: flashState = ''
  plainControlState: flashState = ''
  cryptoControlState: flashState = ''
  keyButtonName: "Save" | "Clear" = "Save"
  fileInfo: FileInfo

  constructor() { }

  ngOnInit() {
    const keyHex = localStorage.getItem("key")
    if (keyHex) {
      this.keyControl.setValue(keyHex)
      this.keyButtonName = "Clear"
      this.keyControlState = "greenflash"
    }
  }

  onDebugClick() { }

  onKeyStoreClick() {
    if (this.keyControl.invalidOrEmpty) {
      this.keyControlState = 'redflash'
      this.keyControl.markAsDirty()
      return
    }
    if (this.keyButtonName === "Save") {
      localStorage.setItem("key", this.keyControl.value)
      this.keyButtonName = "Clear"
      this.keyControlState = "greenflash"
      return
    }

    localStorage.removeItem("key")
    this.keyButtonName = "Save"
    this.keyControlState = "greenflash"
  }

  onBrowseClick(event) {
    var file: File = event.target.files[0]
    if (!file) return

    var fileReader = new FileReader()
    fileReader.onload = (event) => {
      const arrayBuffer: ArrayBuffer = fileReader.result
      const buffer = Buffer.from(arrayBuffer)
      this.fileInfo = new FileInfo(buffer, file.name)
    }
    fileReader.readAsArrayBuffer(file)
  }

  onEncodingChange() {
    switch (this.encoding) {
      case "utf8":
        this.plainControl.clearValidators()
        this.plainControl.enable()
        this.cryptoControl.enable()
        break
      case "hex":
        this.plainControl.setValidators(hexValidator)
        this.plainControl.enable()
        this.cryptoControl.enable()
        break
      case "base64":
        this.plainControl.setValidators(base64Validator)
        this.plainControl.enable()
        this.cryptoControl.enable()
        break
      case "file":
        this.plainControl.disable()
        this.cryptoControl.disable()
        break
    }
    this.plainControl.reset()
  }

  onGenerateClick() {
    if (this.keyButtonName === "Clear") {
      toastr.error("You must clear the saved key before generating a new one")
      return
    }
    this.keyControl.setValue(crypto.randomBytes(32).toString("hex"))
    this.keyControlState = 'greenflash'
    if (this.autoIV) return

    this.ivControl.setValue(crypto.randomBytes(16).toString("hex"))
    this.ivControlState = 'greenflash'
  }

  onEncryptClick() {
    this.cryptoControl.reset()
    if (this.keyControl.invalidOrEmpty) {
      this.keyControlState = 'redflash'
      this.keyControl.markAsDirty()
      return
    }
    if (!this.autoIV && this.ivControl.invalidOrEmpty) {
      this.ivControlState = 'redflash'
      return
    }
    if (this.encoding !== "file" && this.plainControl.invalidOrEmpty) {
      this.plainControlState = 'redflash'
      this.plainControl.markAsDirty()
      return
    }
    if (this.encoding === "file" && !this.fileInfo) {
      this.plainControlState = 'redflash'
      this.plainControl.markAsDirty()
      return
    }

    try {
      const key = Buffer.from(this.keyControl.value, "hex")
      const iv = this.autoIV ? crypto.randomBytes(16) : Buffer.from(this.ivControl.value, "hex")
      const plainData = this.encoding === 'file' ?
        this.fileInfo.data : Buffer.from(this.plainControl.value, this.encoding)
      const cipher = crypto.createCipheriv('aes-256-cbc', key, iv)
      let encryptedData = cipher.update(plainData)
      encryptedData = Buffer.concat([encryptedData, cipher.final()])
      if (this.autoIV) encryptedData = Buffer.concat([encryptedData, iv])

      if (this.encoding === 'file') {
        downloadFile(encryptedData, `${this.fileInfo.fullName}.enc`)
        return
      }

      this.cryptoControl.setValue(encryptedData.toString("hex"))
      this.cryptoControlState = 'greenflash'
    } catch (ex) {
      toastr.error(ex)
    }
  }

  onDecryptClick() {
    this.plainControl.reset()
    if (this.keyControl.invalidOrEmpty) {
      this.keyControlState = 'redflash'
      this.keyControl.markAsDirty()
      return
    }
    if (!this.autoIV && this.ivControl.invalidOrEmpty) {
      this.ivControlState = 'redflash'
      return
    }
    if (this.encoding !== "file" && this.cryptoControl.invalidOrEmpty) {
      this.cryptoControlState = 'redflash'
      this.cryptoControl.markAsDirty()
      return
    }
    if (this.encoding === "file" && !this.fileInfo) {
      this.cryptoControlState = 'redflash'
      this.cryptoControl.markAsDirty()
      return
    }

    try {
      const key = Buffer.from(this.keyControl.value, "hex")
      let iv: Buffer
      let encryptedBuffer = this.encoding === 'file' ?
        this.fileInfo.data : Buffer.from(this.cryptoControl.value, "hex")
      if (this.autoIV) {
        iv = encryptedBuffer.slice(-16)
        encryptedBuffer = encryptedBuffer.slice(0, -16)
      } else {
        iv = Buffer.from(this.ivControl.value, "hex")
      }

      const decipher = crypto.createDecipheriv('aes-256-cbc', key, iv)
      let decryptedData = decipher.update(encryptedBuffer)
      decryptedData = Buffer.concat([decryptedData, decipher.final()])

      if (this.encoding === 'file') {
        downloadFile(decryptedData, this.fileInfo.name)
        return
      }

      this.plainControl.setValue(decryptedData.toString(this.encoding))
      this.plainControlState = 'greenflash'
    } catch (ex) {
      toastr.error(ex)
    }
  }

  onSendClick() {
    if (this.cryptoControl.invalidOrEmpty) {
      this.cryptoControlState = 'redflash'
      this.cryptoControl.markAsDirty()
      return
    }
    window.open(`mailto:?Subject=Message&Body=${this.cryptoControl.value}`, "_parent")
  }
}