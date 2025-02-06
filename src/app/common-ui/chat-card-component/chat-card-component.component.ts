import { Component, inject, Input } from '@angular/core';
import { Profile } from '../../data/Interfaces/profile.interface';
import { ProfileService } from '../../data/services/profile.service';

@Component({
  selector: 'app-chat-card-component',
  imports: [],
  templateUrl: './chat-card-component.component.html',
  styleUrl: './chat-card-component.component.scss'
})
export class ChatCardComponentComponent {
   @Input() profile!: Profile;
   profileService = inject(ProfileService)

   
}
