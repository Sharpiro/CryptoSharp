import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { KeyInfo } from '..';

@Component({
  selector: 'app-add-key-modal',
  templateUrl: './add-key-modal.component.html',
  styleUrls: ['./add-key-modal.component.css']
})
export class AddKeyModalComponent {
  constructor(public dialogRef: MatDialogRef<AddKeyModalComponent>, @Inject(MAT_DIALOG_DATA) public data: KeyInfo) { }
}
