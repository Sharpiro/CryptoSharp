import { Injectable } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material';
import { SpinnerComponent } from './spinner.component';

@Injectable()
export class SpinnerService {
  dialogRef: MatDialogRef<SpinnerComponent, any>;

  constructor(private dialog: MatDialog) { }

  open() {
    if (this.dialogRef) return

    // workaround for bug https://github.com/angular/angular/issues/15634
    setTimeout(() => {
      this.dialogRef = this.dialog.open(SpinnerComponent, {
        disableClose: true,
        panelClass: "clearDialog"
      })
    });
  }

  close() {
    if (!this.dialogRef) return
    this.dialogRef.close()
    this.dialogRef = null
  }
}
