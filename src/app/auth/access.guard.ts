import { inject } from "@angular/core"
import { AuthService } from "./auth.service"
import { Router } from "@angular/router"

export const canActivateAuth = () => {
    const authService = inject(AuthService);
    const router = inject(Router);
  
    const isLoggedIn = authService.isAuth;
    console.log('isLoggedIn:', isLoggedIn); // Log authentication status
  
    if (isLoggedIn) {
      return true;
    }
    return router.createUrlTree(['/login']);
  };
  