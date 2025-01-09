import { Component, input } from '@angular/core';
import { Profile } from '../../data/Interfaces/profile.interface';
import { RouterLink, RouterModule } from '@angular/router';

@Component({
  selector: 'app-profile-header',
  imports: [],
  templateUrl: './profile-header.component.html',
  styleUrl: './profile-header.component.scss'
})
export class ProfileHeaderComponent {
  profile = input<Profile>()
}
