import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatButtonModule, MatToolbarModule, MatInputModule, ShowOnDirtyErrorStateMatcher, ErrorStateMatcher, MatCheckboxModule, MatRadioModule, MatSidenavModule, MatTableModule, MatPaginatorModule, MatSortModule } from '@angular/material';
import { EncryptionComponent } from './encryption/encryption.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { KeyExchangeComponent } from './key-exchange/key-exchange.component';
import { MatIconModule } from '@angular/material/icon';
import { KeyManagementComponent } from './key-management/key-management.component'

@NgModule({
  declarations: [
    AppComponent,
    EncryptionComponent,
    KeyExchangeComponent,
    KeyManagementComponent
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
    MatSortModule
  ],
  providers: [
    { provide: ErrorStateMatcher, useClass: ShowOnDirtyErrorStateMatcher }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }