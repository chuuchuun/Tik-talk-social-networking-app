import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { Profile } from '../../data/Interfaces/profile.interface';
import { ProfileService } from '../../data/services/profile.service';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-post-enter',
  imports: [ReactiveFormsModule],
  templateUrl: './post-enter.component.html',
  styleUrl: './post-enter.component.scss'
})
export class PostEnterComponent {
  @Output() postCreated = new EventEmitter<void>();
  @Input() profile!: Profile;
  profileService = inject(ProfileService)
  me = this.profileService.me
  fb = inject(FormBuilder)
  form = this.fb.group({
    title: [''],
    content: ['']
  })

  CreatePost() {
    this.form.markAllAsTouched();
    this.form.updateValueAndValidity();
  
    if (this.form.invalid) {
      console.log("Invalid data");
      return;
    }

    this.profileService.createPost(this.form.value.title!, this.form.value.content!).subscribe(() => {
      this.postCreated.emit();
    })
    this.form.reset();
  }

  
}
