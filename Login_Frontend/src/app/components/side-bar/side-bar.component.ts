import { Component } from '@angular/core';
import { AuthentifikationService } from '../../services/authentifikation.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-side-bar',
  templateUrl: './side-bar.component.html',
  styleUrl: './side-bar.component.scss'
})
export class SideBarComponent {

  constructor(public auth: AuthentifikationService, private router: Router)
  {
    this.auth = auth; 
    this.router = router;
  }

  logout()
  {
    localStorage.removeItem('token');
    this.router.navigate(['/login']);
  }
}
