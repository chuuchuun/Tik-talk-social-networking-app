import { Component, inject } from '@angular/core';
import { ProfileService } from '../../data/services/profile.service';
import { Profile } from '../../data/Interfaces/profile.interface';
import { Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { ProfileCardComponent } from '../../common-ui/profile-card/profile-card.component';
import { AsyncPipe, NgFor, NgForOf, NgIf } from '@angular/common';

@Component({
  selector: 'app-search-page',
  imports: [ProfileCardComponent],
  templateUrl: './search-page.component.html',
  styleUrl: './search-page.component.scss',
})
export class SearchPageComponent {
  profileService: ProfileService = inject(ProfileService);
  me = this.profileService.me; // Current user's profile
  profiles: Profile[] = []; // List of profiles

  constructor() {
    // Fetch the list of test accounts
    this.profileService.getTestAccounts().subscribe((val) => {
      this.profiles = val;
    });
  }

  // Method to check if the current profile is subscribed
  
}
