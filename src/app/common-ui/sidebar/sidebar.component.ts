import { Component } from '@angular/core';
import { SvgIconComponent } from '../svg-icon/svg-icon.component';
import { NgFor } from '@angular/common';
import { SubscriberCardComponent } from "./subscriber-card/subscriber-card.component";
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-sidebar',
  imports: [SvgIconComponent, NgFor, SubscriberCardComponent, RouterLink],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent {
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
}
