import { Component, Input } from '@angular/core';
import { Post } from '../../data/Interfaces/post.interface';
import { Profile } from '../../data/Interfaces/profile.interface';

@Component({
  selector: 'app-post',
  imports: [],
  templateUrl: './post.component.html',
  styleUrl: './post.component.scss'
})
export class PostComponent {
  @Input() post!: Post;
  @Input() author!: Profile
}
