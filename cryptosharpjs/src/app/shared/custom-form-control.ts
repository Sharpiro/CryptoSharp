import { FormControl, AbstractControl, ValidationErrors } from '@angular/forms'

const hexPattern = new RegExp('^[a-fA-F0-9]+$')
const base64Pattern = new RegExp('^(?:[A-Za-z0-9+\/]{4})*(?:[A-Za-z0-9+\/]{2}==|[A-Za-z0-9+\/]{3}=)?$')

export class CustomFormControl extends FormControl {
    get invalidOrEmpty(): boolean {
        return !this.value || this.invalid
    }
}

export function hexValidator(control: AbstractControl): ValidationErrors | null {
    const errorLabel = 'hex'
    if (!control.value) return null
    if (control.value.length % 2 !== 0) return { [errorLabel]: 'invalid hex length' }
    if (!hexPattern.test(control.value)) return { [errorLabel]: 'invalid hex characters' }
}

export function base64Validator(control: AbstractControl): ValidationErrors | null {
    const errorLabel = 'base64'
    if (!control.value) return null
    if (!base64Pattern.test(control.value)) return { [errorLabel]: 'invalid base64 characters' }
}