import { Component, inject } from '@angular/core';
import { SvgIconComponent } from '../svg-icon/svg-icon.component';
import { AsyncPipe, JsonPipe, NgFor } from '@angular/common';
import { SubscriberCardComponent } from "./subscriber-card/subscriber-card.component";
import { RouterLink } from '@angular/router';
import { ProfileService } from '../../data/services/profile.service';
import { firstValueFrom } from 'rxjs';
import { Profile } from '../../data/Interfaces/profile.interface';

@Component({
  selector: 'app-sidebar',
  imports: [SvgIconComponent, NgFor, SubscriberCardComponent, RouterLink, AsyncPipe, JsonPipe],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent {
  profileService = inject(ProfileService)
  subscribers$ = this.profileService.getMySubscribersShortList()
  me =this.profileService.me
  menuItems = [
    {
      label: 'My page',
      icon: 'home',
      link: ''
    },
    {
      label: 'Chats',
      icon: 'chats',
      link: 'chats'
    },
    {
      label: 'Search',
      icon: 'search',
      link: 'search'
    }
  ]

  ngOnInit(){
    firstValueFrom(this.profileService.getMe())
  }

  getRouterLink(label: string): string {
    switch (label) {
      case 'My page':
        return 'profile/me';
      case 'Chats':
        return 'chats';
      case 'Search':
        return 'search';
      default:
        return '/';
    }
  }
}
