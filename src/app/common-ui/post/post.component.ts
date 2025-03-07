import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { Post } from '../../data/Interfaces/post.interface';
import { Profile } from '../../data/Interfaces/profile.interface';
import { DatePipe, NgStyle } from '@angular/common';
import { ProfileService } from '../../data/services/profile.service';
import { delay } from 'rxjs';

@Component({
  selector: 'app-post',
  providers: [DatePipe],
  templateUrl: './post.component.html',
  styleUrl: './post.component.scss',
  imports: [NgStyle]
})
export class PostComponent {
  @Input() post!: Post;
  @Input() author!: Profile
  @Output() modified = new EventEmitter<void>();
  contextMenuVisible = false;
  contextMenuX = 0;
  contextMenuY = 0;
  profileService = inject(ProfileService);

  openContextMenu(button: HTMLElement) {
    this.contextMenuVisible = true;
  
    const rect = button.getBoundingClientRect(); // Get button position
    this.contextMenuX = rect.left; // Align left
    this.contextMenuY = rect.bottom + window.scrollY; // Place below the button
  }
  

  deletePost(post_id: number) {
    console.log("Delete post:", post_id);
    this.contextMenuVisible = false;
  
    this.profileService.deletePost(post_id)
          .pipe(delay(500)) // Delay refresh by 500ms to avoid premature UI update
          .subscribe({
            next: () => {
              console.log("Message deleted successfully");
              this.modified.emit();
            },
            error: err => {
              console.error("Error deleting message:", err);
            }
          });
      }
      
    
      editMessage(post: Post) {
        console.log("Edit post:", post);
        this.contextMenuVisible = false;
        // Implement edit functionality (e.g., open a modal)
      }
    
  constructor(private datePipe: DatePipe) {
    const formattedDate = this.datePipe.transform(new Date(), 'short');
    console.log(formattedDate);
  }

  formatDate(date: Date): string {
    const now = new Date();
    const postDate = new Date(date);
  
    const diffInMs = now.getTime() - postDate.getTime();
    const diffInSeconds = Math.floor(diffInMs / 1000);  // Convert to seconds
    const diffInMinutes = Math.floor(diffInSeconds / 60);  // Convert to minutes
    const diffInHours = Math.floor(diffInMinutes / 60);  // Convert to hours
  
    if (postDate.toDateString() === now.toDateString()) {
      if (diffInHours > 0) return `${diffInHours} hours ago`;
      if (diffInMinutes > 0) return `${diffInMinutes} minutes ago`;
      if (diffInSeconds > 15) return `${diffInSeconds} seconds ago`;
      return 'Just now';
    }
  
    if (postDate.getFullYear() === now.getFullYear()) {
      return this.datePipe.transform(postDate, 'dd.MM') || '';
    }
  
    return this.datePipe.transform(postDate, 'dd.MM.yyyy') || '';
  }
  
  
}
