import { Routes } from '@angular/router';
import { Login } from './components/auth/login/login';
import { NewUser } from './components/auth/new-user/new-user';
import { Home } from './components/home/home';
import { authGuard } from './guards/auth-guard';
import { StartYourOwn } from './components/start-your-own/start-your-own';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component:  Login},
  { path: 'register', component: NewUser },
  { path: 'home', component: Home, canActivate: [authGuard] },
  { path: 'start-own', component: StartYourOwn}

];