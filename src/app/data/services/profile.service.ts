import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';
import { Profile } from '../Interfaces/profile.interface';
import { Pageble } from '../Interfaces/pageble.interface';
import {Chat} from '../Interfaces/chat.interface';
import { BehaviorSubject, map, pipe, single, tap } from 'rxjs';
import { Message } from '../Interfaces/message.interface';
import { Post } from '../Interfaces/post.interface';

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
    console.log('File:', file);
    const fd = new FormData();
        if (file) {
      fd.append('file', file);
      console.log('FormData:', fd.get('file'));
    } else {
      console.log('No file provided');
    }
    
    return this.http.post<Profile>(`${this.baseApiUrl}/account/me/upload-image`, fd, {
      withCredentials: true
    });
  }
  
 

  getMyChats(): Observable<Chat[]> {
    return this.http.get<Chat[]>(`${this.baseApiUrl}/chat/get_my_chats`).pipe(
      tap(chats => this.chatsSubject.next(chats))
    );
  }

  filterChats(params: Record<string, any>){
    return this.http.get<Chat[]>(`${this.baseApiUrl}/chat/get_my_chats`,
      {
        params
      }
    ).pipe(
      tap(chats => this.chatsSubject.next(chats))
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
        null,
        { 
          headers: { 'Content-Type': 'application/json' },
          withCredentials: true
        }
    ).pipe(
      tap(() => this.getMyChats().subscribe())
    );
  }

  getMessages(chatId: number): Observable<Message[]> {
    return this.http.get<Message[]>(`${this.baseApiUrl}/chat/${chatId}/messages`);
  }

  deleteMessage(chat_id: number, message_id: number): Observable<any> {
    return this.http.delete<Observable<any>>(`${this.baseApiUrl}/message/${chat_id}/${message_id}`)
  }

  getPosts(profile_id: number): Observable<Post[]> {
    return this.http.get<Post[]>(`${this.baseApiUrl}/post`, {
      params: new HttpParams().set('user_id', profile_id)
    });
  }
  
  createPost(title: string, content: string): Observable<Post> {
    const userId = this.me()?.id; 
    return this.http.post<Post>(`${this.baseApiUrl}/post`, {
      content,
      authorId: userId
    });
  }
  
  deletePost(post_id : number){
    return this.http.delete<Observable<any>>(`${this.baseApiUrl}/post/${post_id}`)
  }
}
