import { Component, inject, Input } from '@angular/core';
import { Profile } from '../../data/Interfaces/profile.interface';
import { ProfileService } from '../../data/services/profile.service';

@Component({
  selector: 'app-profile-card',
  imports: [],
  templateUrl: './profile-card.component.html',
  styleUrl: './profile-card.component.scss'
})
export class ProfileCardComponent {
  @Input() profile!: Profile;
  profileService = inject(ProfileService)
  Subscribe(id: number) {
    console.log("Trying to call API to subscribe with ID:", id);
  
    this.profileService.subscribeToProfile(id).subscribe({
      next: (response) => {
        console.log("Subscription successful:", response);
        // Optionally update the UI or notify the user of the successful subscription
      },
      error: (err) => {
        console.error("Error occurred during subscription:", err);
      }
    });
  }
  
}
