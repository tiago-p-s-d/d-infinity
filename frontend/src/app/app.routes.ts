import { Routes } from '@angular/router';
import { Login } from './components/auth/login/login';
import { NewUser } from './components/auth/new-user/new-user';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component:  Login},
  { path: 'register', component: NewUser },

];