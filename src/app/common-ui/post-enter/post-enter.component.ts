import { Component, Input } from '@angular/core';
import { Profile } from '../../data/Interfaces/profile.interface';

@Component({
  selector: 'app-post-enter',
  imports: [],
  templateUrl: './post-enter.component.html',
  styleUrl: './post-enter.component.scss'
})
export class PostEnterComponent {
  @Input() profile!: Profile;
}
