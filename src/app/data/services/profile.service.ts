import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';
import { Profile } from '../Interfaces/profile.interface';
import { Pageble } from '../Interfaces/pageble.interface';
import { map, single, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  http:HttpClient = inject(HttpClient)
  baseApiUrl: string = 'http://localhost:5178/api'
  me = signal<Profile | null>(null)
  getTestAccounts(){
      //return this.http.get<Profile[]>(`${this.baseApiUrl}account/test_accounts`)
      return this.http.get<Profile[]>(`${this.baseApiUrl}/account`)
    }

  getMe(){
    return this.http.get<Profile>(`${this.baseApiUrl}/account/me`, {
      withCredentials: true
    })
    .pipe(
      tap(res=>{
        this.me.set(res)
      })
    )
  }
  getSubscribersShortList(){
    return this.http.get<Pageble<Profile>>(`${this.baseApiUrl}/account/subscribers`)
    .pipe(
      map(res => res.items.slice(0,3))
    )
  }
}
