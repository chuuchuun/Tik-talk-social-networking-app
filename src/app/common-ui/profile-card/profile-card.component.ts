import { Component, inject, Input } from '@angular/core';
import { Profile } from '../../data/Interfaces/profile.interface';
import { ProfileService } from '../../data/services/profile.service';
import { Observable } from 'rxjs';
import { AsyncPipe, NgFor, NgForOf, NgIf } from '@angular/common';
import { Pageble } from '../../data/Interfaces/pageble.interface';
import { toObservable } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-profile-card',
  imports: [AsyncPipe, NgIf, NgFor, NgForOf, RouterLink],
  templateUrl: './profile-card.component.html',
  styleUrl: './profile-card.component.scss'
})
export class ProfileCardComponent {
  @Input() profile!: Profile;
  profileService = inject(ProfileService)
  me = this.profileService.me

  subscriptions: Profile[] = [];
  ngOnInit(){
    this.profileService.getMySubscriptionsShortList().subscribe((subs) => {
      this.subscriptions = subs;
    });
    console.log(this.subscriptions);
    
  }


  isSubscribed(profileId: number): boolean {
    return this.subscriptions.some((sub) => sub.id == profileId);
  }

  Subscribe(id: number) {
    console.log("Trying to call API to subscribe with ID:", id);
  
    this.profileService.subscribeToProfile(id).subscribe({
      next: (response) => {
        console.log("Subscription successful:", response);
        // Re-fetch subscriptions to update the UI with the new state
        this.loadSubscriptions(); 
      },
      error: (err) => {
        console.error("Error occurred during subscription:", err);
      }
    });
  }
  
  loadSubscriptions() {
    // Re-fetch subscriptions from the server to reflect the change
    this.profileService.getMySubscriptionsShortList().subscribe((subs) => {
      this.subscriptions = subs;
      console.log("Updated subscriptions:", this.subscriptions);
    });
  }
  
}
