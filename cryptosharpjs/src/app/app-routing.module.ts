import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { EncryptionComponent } from './encryption/encryption.component';
import { KeyExchangeComponent } from './key-exchange/key-exchange.component';

const routes: Routes = [
  { path: "encryption", component: EncryptionComponent },
  { path: "keyexchange", component: KeyExchangeComponent },
  { path: '**', redirectTo: '/encryption', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
