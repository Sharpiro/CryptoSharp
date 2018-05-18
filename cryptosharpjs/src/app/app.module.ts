import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatButtonModule, MatToolbarModule, MatInputModule, ShowOnDirtyErrorStateMatcher, ErrorStateMatcher, MatCheckboxModule, MatRadioModule, MatSidenavModule, MatTableModule, MatPaginatorModule, MatSortModule, MatDialogModule, MatProgressSpinnerModule } from '@angular/material';
import { EncryptionComponent } from './encryption/encryption.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { KeyExchangeComponent } from './key-exchange/key-exchange.component';
import { MatIconModule } from '@angular/material/icon';
import { KeyManagementComponent } from './key-management/key-management.component';
import { AddKeyModalComponent } from './key-management/add-key-modal/add-key-modal.component';
import { SpinnerComponent } from './shared/spinner/spinner.component'
import { KeyManagementService } from './key-management/key-management.service';
import { SpinnerService } from './shared/spinner/spinner.service';

@NgModule({
  declarations: [
    AppComponent,
    EncryptionComponent,
    KeyExchangeComponent,
    KeyManagementComponent,
    AddKeyModalComponent,
    SpinnerComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    MatButtonModule,
    MatToolbarModule,
    MatInputModule,
    FormsModule,
    ReactiveFormsModule,
    MatCheckboxModule,
    MatRadioModule,
    MatIconModule,
    MatSidenavModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatDialogModule,
    MatProgressSpinnerModule
  ],
  providers: [
    { provide: ErrorStateMatcher, useClass: ShowOnDirtyErrorStateMatcher },
    KeyManagementService,
    SpinnerService
  ],
  entryComponents: [AddKeyModalComponent, SpinnerComponent],
  bootstrap: [AppComponent]
})
export class AppModule { }