import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, tap, throwError } from 'rxjs';
import { TokenResponse } from './auth.interface';
import { CookieService } from 'ngx-cookie-service';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  token: string | null = null
  refreshToken: string | null = null
  http: HttpClient = inject(HttpClient)
  baseApiUrl: string = 'http://localhost:5178/api'
  cookiesService = inject(CookieService)
  router = inject(Router)

  get isAuth(){
    if(!this.token){
      this.token = this.cookiesService.get('token')
      this.refreshToken = this.cookiesService.get('refreshToken')
    } 
    console.log('Access token:', this.token); // Log the token to verify it's being set
    console.log('Refresh token:', this.refreshToken); // Log the token to verify it's being set
    return !!this.token
  }

  login(payload: { username: string, password: string }) {
    return this.http.post<TokenResponse>('http://localhost:5178/api/auth/login', payload)
      .pipe(
        tap(val => this.saveTokens(val))
      );
  }
  
  refreshAuthToken(){
    return this.http.post<TokenResponse>('http://localhost:5178/api/auth/refresh',
      {
        accessToken : this.token,
        refreshToken : this.refreshToken
      },
    ).pipe(
      tap(res=> this.saveTokens(res)),
      catchError(err => {
        this.logout()
        return throwError(() => err)
      })
    )
  }
  
  logout() {
    console.log("Inside logout method");
  
    // Clear cookies
    this.cookiesService.deleteAll();
    console.log("Cookies cleared");
  
    // Call logout API
    this.http.post<String>(`${this.baseApiUrl}/auth/logout`, {
      accessToken: this.token, 
      refreshToken: this.refreshToken
    }).subscribe({
      next: (response) => {
        console.log("Logout API response:", response);
      },
      error: (err) => {
        console.error("Logout error:", err);
      }
    });
  
    // Reset tokens and navigate to login page
    this.token = null;
    this.refreshToken = null;
    this.router.navigate(['/login']);
  }
  
saveTokens(response : TokenResponse) {
    console.log('Login response:', response); // Check the response here
    this.token = response.accessToken;
    this.refreshToken = response.refreshToken;

    this.cookiesService.set('token', this.token)
    this.cookiesService.set('refreshToken', this.refreshToken)
  }
}
