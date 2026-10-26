import Header from "@/widgets/Header"
import Footer from "@/widgets/Footer"
import styles from './LogginingPage.module.css'
import {useState } from "react"
import LoginForm from "@/features/login"
import RegisterForm from "@/features/register"
const LogginingPage = ()=>{
    const [isLogin, setIsLogin] = useState(true)
    const [error,setError] = useState('')
    const [showAlert,setShowAlert] = useState(false)
    const LogginingButtonHandler = ({target})=>{
        if(target.id == 'Login'){
            if(!isLogin){
                setIsLogin(!isLogin)
            }
        } else if(target.id == 'Register'){
            if(isLogin){
                setIsLogin(!isLogin)
            }
        }
        setShowAlert(false)
    }
    return(
        <>
            <Header isLoginDisabled = {true}></Header>
                <section className={`page-content ${styles['loggining-content']}`}>
                    {isLogin ?
                    <>
                        <div className={`${styles['loggining-alert']} ${showAlert ? (error? styles['error']:styles['success']): ''}`}
                        >
                            {error? error : 'Успешный вход'}
                        </div>
                        <LoginForm styles={styles} setFormError={setError} setShowAlert={setShowAlert}/>
                    </>
                    :
                    <>
                        <div className={`${styles['loggining-alert']} ${showAlert ? (error? styles['error']:styles['success']): ''}`}
                        >
                            {error? error : 'Вы успешно зарегистрировались'}
                        </div>
                        <RegisterForm 
                        styles={styles}
                        setShowAlert={setShowAlert} 
                        setFormError={setError}/>
                    </>}
                    
                    <div className={styles["loggining-navigation"]}>
                        <span
                        onClick={LogginingButtonHandler} 
                        className={`${styles['loggining-navigation__button']} ${isLogin ? styles['is-active'] : ''}`}
                        id="Login"
                        >
                            Вход
                        </span>
                        <span 
                        onClick={LogginingButtonHandler}
                        className={`${styles['loggining-navigation__button']} ${!isLogin ? styles['is-active'] : ''}`} 
                        id="Register"
                        >
                            Регистрация
                        </span>
                    </div>
                </section>
            <Footer/>
        </>
        
    )
}
export default LogginingPage