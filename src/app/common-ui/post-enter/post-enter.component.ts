import { Component, inject, Input } from '@angular/core';
import { Profile } from '../../data/Interfaces/profile.interface';
import { ProfileService } from '../../data/services/profile.service';

@Component({
  selector: 'app-post-enter',
  imports: [],
  templateUrl: './post-enter.component.html',
  styleUrl: './post-enter.component.scss'
})
export class PostEnterComponent {
  @Input() profile!: Profile;
  profileService = inject(ProfileService)
  me = this.profileService.me

}
