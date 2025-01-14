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
  getMySubscribersShortList(subsAmount = 3){
    return this.http.get<Pageble<Profile>>(`${this.baseApiUrl}/account/subscribers`)
    .pipe(
      map(res => res.items.slice(0,subsAmount))
    )
  }

  getSubscribersShortList(id: number, subsAmount = 3){
    return this.http.get<Pageble<Profile>>(`${this.baseApiUrl}/account/${id}/subscribers`)
    .pipe(
      map(res => res.items.slice(0,subsAmount))
    )
  }

  getSubscribersAmount(id: number){
    return this.http.get<Pageble<Profile>>(`${this.baseApiUrl}/account/${id}/subscribers`)
    .pipe(
      map(res => res.total)
    )
  }
  getAccount(id : number){
    return this.http.get<Profile>(`${this.baseApiUrl}/account/${id}`)
  }

  patchProfile(profile: Partial<Profile>) {
    return this.http.patch<Profile>(`${this.baseApiUrl}/account/me`, profile); // Ensure this returns an observable
  }
  
  subscribeToProfile(id: number) {
    console.log("Calling API with ID:", id);
    
    // Ensure the payload is sent as JSON (wrapping the `id`)
    return this.http.post<string>(`${this.baseApiUrl}/account/subscribe`, { id });
  }

  getMySubscriptionsShortList(){
    return this.http.get<Pageble<Profile>>(`${this.baseApiUrl}/account/subscriptions`)
    .pipe(
      map(res => res.items)
    )
  }
  
}
