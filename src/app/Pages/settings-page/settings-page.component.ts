import { Component, inject } from '@angular/core';
import { ProfileHeaderComponent } from "../../common-ui/profile-header/profile-header.component";
import { ProfileService } from '../../data/services/profile.service';
import { toObservable } from '@angular/core/rxjs-interop';
import { ActivatedRoute } from '@angular/router';
import { interval, Subscription, switchMap, tap } from 'rxjs';
import { AsyncPipe } from '@angular/common';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Profile } from '../../data/Interfaces/profile.interface';
import { AvatarUploadComponent } from './avatar-upload/avatar-upload.component';
@Component({
  selector: 'app-settings-page',
  imports: [ProfileHeaderComponent, AsyncPipe, ReactiveFormsModule, AvatarUploadComponent ],
  templateUrl: './settings-page.component.html',
  styleUrl: './settings-page.component.scss'
})
export class SettingsPageComponent {
profileService = inject(ProfileService)
  me$ = toObservable(this.profileService.me)
  profile$ = this.profileService.me
  fb = inject(FormBuilder)
  form = this.fb.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    username: [{value: '', disabled: true},Validators.required],
    description: [''],
    stack: [['']]
  }) 
  onSave() {    
    
    this.form.markAllAsTouched();
    this.form.updateValueAndValidity();
  
    if (this.form.invalid) {
      console.log("Save invalid");
      return;
    }
    console.log("Form valid");
    
    //@ts-ignore
    this.profileService.patchProfile({
      ...this.form.value,
      stack: this.splitStack(this.form.value.stack)
    }).subscribe({
      next: (updatedProfile) => {
        console.log("Profile updated successfully:", updatedProfile);
        this.form.patchValue(updatedProfile); 
        this.profileService.me.set(updatedProfile)
      },
      error: (err) => {
        console.error("Error updating profile:", err);
      }
    });
  }
  
  onDiscard(){
    this.me$.pipe(
      tap((profile: Profile | null) => {
        if (profile) {
          this.form.patchValue({
            firstName: profile.firstName,
            lastName: profile.lastName,
            username: profile.username, // Set username from the observable
            description: profile.description,
            stack: profile.stack
          });
        }
      })
    ).subscribe();
  }
  constructor() {
    this.me$.pipe(
      tap((profile: Profile | null) => {
        if (profile) {
          this.form.patchValue({
            firstName: profile.firstName,
            lastName: profile.lastName,
            username: profile.username, // Set username from the observable
            description: profile.description,
            //@ts-ignore
            stack: this.mergeStack(profile.stack)
          });
        }
      })
    ).subscribe();
  }

  splitStack(stack: string | null | string[] | undefined){
    if(Array.isArray(stack)) return stack
    if(!stack) return []

    return stack.split(',')
  }

  mergeStack(stack: string | null | string[]){
    if(Array.isArray(stack)) return stack.join(',')
    if(!stack) return ''

    return stack
  }
}
