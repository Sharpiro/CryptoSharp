import { Injectable } from '@angular/core';
import { KeyInfo } from '.';
import { Observable } from 'rxjs';

@Injectable()
export class KeyManagementService {

  constructor() {

   }

  getKeyInfoList(): Observable<KeyInfo[]> {
    return Observable.of(keyInfoList.map((d, i) => {
      d.id = i + 1
      return d
    })).delay(2000)
  }

  setKeyInfoList(entries: KeyInfo[]): Observable<any> {
    keyInfoList = entries
    return Observable.of().delay(1000)
  }
}

let keyInfoList: KeyInfo[] = [
  { key: "aabb", name: "key1", description: "key1D" },
  { key: "aacc", name: "key2", description: "key2D" },
  { key: "aadd", name: "key2", description: "key3D" },
  { key: "aabb", name: "key1", description: "key1D" },
  { key: "aacc", name: "key2", description: "key2D" },
  { key: "aadd", name: "key2", description: "key3D" },
  { key: "aabb", name: "key1", description: "key1D" },
  { key: "aacc", name: "key2", description: "key2D" },
  { key: "aadd", name: "key2", description: "key3D" },
  { key: "aabb", name: "key1", description: "key1D" },
  { key: "aacc", name: "key2", description: "key2D" },
  { key: "aadd", name: "key2", description: "key3D" },
  { key: "aabb", name: "key1", description: "key1D" },
  { key: "aacc", name: "key2", description: "key2D" },
  { key: "aadd", name: "key2", description: "key3D" },
  { key: "aabb", name: "key1", description: "key1D" },
  { key: "aacc", name: "key2", description: "key2D" },
  { key: "aadd", name: "key2", description: "key3D" },
]