import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';
import { Profile } from '../Interfaces/profile.interface';
import { Pageble } from '../Interfaces/pageble.interface';
import {Chat} from '../Interfaces/chat.interface';
import { BehaviorSubject, map, pipe, single, tap } from 'rxjs';
import { Message } from '../Interfaces/message.interface';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  http:HttpClient = inject(HttpClient)
  baseApiUrl: string = 'http://localhost:5178/api'
  me = signal<Profile | null>(null)
  filteredProfiles = signal<Profile[]>([])
  private chatsSubject = new BehaviorSubject<Chat[]>([]);
  chats$ = this.chatsSubject.asObservable(); 
  getTestAccounts(){
      //return this.http.get<Profile[]>(`${this.baseApiUrl}account/test_accounts`)
      return this.http.get<Pageble<Profile>>(`${this.baseApiUrl}/account`)
        .pipe(
          map(res => res.items)
        )
    }

  getMe(){
    return this.http.get<Profile>(`${this.baseApiUrl}/account/me`, {
      withCredentials: true
    })
    .pipe(
      tap(res=>{
        this.me.set(res)
      })
    )
  }
  getMySubscribersShortList(subsAmount = 3){
    return this.http.get<Pageble<Profile>>(`${this.baseApiUrl}/account/subscribers`)
    .pipe(
      map(res => res.items.slice(0,subsAmount))
    )
  }

  getSubscribersShortList(id: number, subsAmount = 3){
    return this.http.get<Pageble<Profile>>(`${this.baseApiUrl}/account/${id}/subscribers`)
    .pipe(
      map(res => res.items.slice(0,subsAmount))
    )
  }

  getSubscribersAmount(id: number){
    return this.http.get<Pageble<Profile>>(`${this.baseApiUrl}/account/${id}/subscribers`)
    .pipe(
      map(res => res.total)
    )
  }
  getAccount(id : number){
    return this.http.get<Profile>(`${this.baseApiUrl}/account/${id}`)
  }

  patchProfile(profile: Partial<Profile>) {
    return this.http.patch<Profile>(`${this.baseApiUrl}/account/me`, profile);
  }
  
  subscribeToProfile(id: number) {
    return this.http.post<string>(`${this.baseApiUrl}/account/subscribe`, { id });
  }

  getMySubscriptionsShortList(){
    return this.http.get<Pageble<Profile>>(`${this.baseApiUrl}/account/subscriptions`)
    .pipe(
      map(res => res.items)
    )
  }

  uploadImage(file: File) {
    console.log('File:', file);  // Log the file object to see if it is being passed correctly
    const fd = new FormData();
    
    // Check if file is valid
    if (file) {
      fd.append('file', file);
      console.log('FormData:', fd.get('file')); // Log the FormData to verify the file is appended
    } else {
      console.log('No file provided');
    }
    
    return this.http.post<Profile>(`${this.baseApiUrl}/account/me/upload-image`, fd, {
      withCredentials: true
    });
  }
  
 

  getMyChats(): Observable<Chat[]> {
    return this.http.get<Chat[]>(`${this.baseApiUrl}/chat/get_my_chats`).pipe(
      tap(chats => this.chatsSubject.next(chats)) // Update BehaviorSubject
    );
  }

  filterProfiles(params: Record<string, any>){
    return this.http.get<Pageble<Profile>>(`${this.baseApiUrl}/account`,
      {
        params
      }
    )
    .pipe(
      tap(res => this.filteredProfiles.set(res.items))
    )
  }
  sendMessage(text: string, chat_id: number) {
    return this.http.post(
        `${this.baseApiUrl}/message/${chat_id}?text=${encodeURIComponent(text)}`, 
        null,  // No request body, text is in query parameters
        { 
          headers: { 'Content-Type': 'application/json' },
          withCredentials: true
        }
    ).pipe(
      tap(() => this.getMyChats().subscribe()) // Refresh chats after sending messag
    );

}


getMessages(chatId: number): Observable<Message[]> {
  return this.http.get<Message[]>(`${this.baseApiUrl}/chat/${chatId}/messages`);
}

}
