import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';
import { Profile } from '../Interfaces/profile.interface';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  http:HttpClient = inject(HttpClient)
  baseApiUrl: string = 'http://localhost:5178/api'
  getTestAccounts(){
      //return this.http.get<Profile[]>(`${this.baseApiUrl}account/test_accounts`)
      return this.http.get<Profile[]>(`${this.baseApiUrl}/account`)
    }

  getMe(){
    return this.http.get<Profile>(`${this.baseApiUrl}/account/me`)
  }
}
