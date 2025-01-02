import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { tap } from 'rxjs';
import { TokenResponse } from './auth.interface';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  token: string | null = null
  refreshToken: string | null = null
  http: HttpClient = inject(HttpClient)
  baseApiUrl: string = 'http://localhost:5178/api'
  cookiesService = inject(CookieService)
  get isAuth(){
    console.log('Access token:', this.token); // Log the token to verify it's being set
    if(!this.token){
      this.token = this.cookiesService.get('token')
    } 
      return !!this.token
  }

  login(payload: { username: string, password: string }) {
    return this.http.post<TokenResponse>('http://localhost:5178/api/auth/login', payload)
      .pipe(
        tap((response: TokenResponse) => {
          console.log('Login response:', response); // Check the response here
          this.token = response.accessToken;
          this.refreshToken = response.refreshToken;

          this.cookiesService.set('token', this.token)
          this.cookiesService.set('refreshToken', this.refreshToken)
        })
      );
  }
  
  

}
