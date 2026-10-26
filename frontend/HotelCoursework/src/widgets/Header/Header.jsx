import Button from '@/shared/components/Button'
import styles from  './Header.module.css'
import RouterLink from '@/shared/components/RouterLink/RouterLink'
import { useContext } from 'react'
import { UserContext } from '@/entities/user'
const Header = (props)=>{
    const {isLoginDisabled = false} = props
    const {user} = useContext(UserContext)
    return(
        <header className={styles["header"]}>
        <div className={styles["header__content"]}>
            <RouterLink className={styles["logo"]} to="/">
            <img className={styles["logo__image"]} src="/five-stars.png" alt="logo"/>
            <div className={styles["logo__name"]}> 
                <div className={styles["logo__name-text"]}>Hotel</div>
                <div className={styles["logo__name-text"]}>Coursework</div>
            </div>
        </RouterLink>
        <nav className={styles["header__navigation"]}>
            <div className={styles["header__nav-links"]}>
                <RouterLink className={styles["header__nav-link"]} to="/">О нас</RouterLink>
                <RouterLink className={styles["header__nav-link"]} to="/">Контакты</RouterLink>
            </div>
            <RouterLink to={user ? `rent-history/${user.id}`:'/loggining'}>
                <Button className={styles["header__nav-button"]} isDisabled ={isLoginDisabled}
                >
                    {user? 'Профиль' : 'Войти'}
                </Button>
            </RouterLink>
            
            <Button className={styles["header__nav-burger"]}>☰</Button>
        </nav>
        </div>
        </header>   
    )
}

export default Header