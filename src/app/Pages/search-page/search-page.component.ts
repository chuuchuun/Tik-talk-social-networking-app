import { Component, inject } from '@angular/core';
import { ProfileService } from '../../data/services/profile.service';
import { Profile } from '../../data/Interfaces/profile.interface';
import { Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { ProfileCardComponent } from '../../common-ui/profile-card/profile-card.component';
import { AsyncPipe, NgFor, NgForOf, NgIf } from '@angular/common';
import { ProfileFiltersComponent } from "./profile-filters/profile-filters.component";

@Component({
  selector: 'app-search-page',
  imports: [AsyncPipe, ProfileCardComponent, ProfileFiltersComponent],
  templateUrl: './search-page.component.html',
  styleUrl: './search-page.component.scss',
})
export class SearchPageComponent {
  profileService: ProfileService = inject(ProfileService);
  me = this.profileService.me;
  profiles = this.profileService.filteredProfiles
  constructor() {
   
  }

  
}
