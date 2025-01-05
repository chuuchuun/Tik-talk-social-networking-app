import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms'; // Import ReactiveFormsModule
import { AuthService } from '../../auth/auth.service';
import { from, map } from 'rxjs';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-login-page',
  imports: [ReactiveFormsModule, RouterModule],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.scss'
})
export class LoginPageComponent {
  authService = inject(AuthService)
  router = inject(Router)

  isPasswordVisible = signal<boolean>(false)
  form = new FormGroup({
    username: new FormControl<string | null>(null, Validators.required),
    password: new FormControl <string | null>(null, Validators.required)
  })

  onSubmit() {
    if (this.form.valid) {
      //@ts-ignore
      this.authService.login(this.form.value).subscribe(res =>{
      this.router.navigate([''])
      console.log(res)
      });
    }
  }
    
}
