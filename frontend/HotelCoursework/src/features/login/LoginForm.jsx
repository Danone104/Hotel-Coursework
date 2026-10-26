import { useContext, useState } from "react"
import Field from "@/shared/components/Field"
import Button from "@/shared/components/Button"
import { UserContext } from "@/entities/user"
import { loginApi } from "../../shared/api"

const LoginForm = (props)=>{
    const {styles,setFormError,setShowAlert}= props
    const [loginformData, setLoginFormData] = useState({
        login: '',
        password: ''
    })
    const [errors, setErrors] = useState({
        login: '',
        password:'',
    });
    const {
        setUser,
        user
    }= useContext(UserContext)
    const handleLoginFormData = ({target})=>{
        setLoginFormData(prev => ({ ...prev, [target.id]: target.value.trimEnd() }))
        setErrors(prev => ({ ...prev, [target.id]: '' }))
        if(target.value.trimEnd() && target.value.startsWith(' ')){
            setErrors(prev => ({ ...prev, [target.id]: 'Не может начинаться с пробела' }))
        }else if(target.id==='login'){
            if(target.value.trimEnd() && target.value.length<3) {
                setErrors(prev => ({ ...prev, 'login': 'Логин должен содержать минимум 3 символа' }))
            }
        }
    }
    const handleFormChange = ()=>{
        setFormError('')
        setShowAlert(false)
    }
    const handleSubmitForm = async (event)=>{
        event.preventDefault()
        if(user){
            setFormError('Вы уже в аккаунте')
        }
        else{
            let isError=false   
            for (let field in loginformData ) {
                if(loginformData[field]===''){
                    setFormError('Все поля должны быть заполнены')
                    isError=true
                    
                }
            }
            if(!isError){   
                for (let error in errors) {
                if(errors[error]!==''){
                    setFormError('Не все поля заполнены корректно')
                }
            }}
            if(!isError){
                const response = await loginApi.login(loginformData.login, loginformData.password);

                if(response.message) {
                    setFormError(response.message);
                } else {
                    const userServer = await loginApi.getProfile();
                    if(userServer.role !== 'Client') {
                        setFormError('Неправильный логин или пароль');
                        await loginApi.logout();
                    }    
                    else{
                        console.log(userServer)
                        setUser(userServer)
                    }
                }
            }

            
            
            }
        setTimeout(()=>{
            setShowAlert(true)
            if(!isError){
                setLoginFormData({
                login:'',
                password: ''
            })}
        },400)
        
        
    }
    return(
        <form className={styles['login-form']} onSubmit={handleSubmitForm} onChange={handleFormChange}>
            <span className={styles['form-label']}>Вход</span>
            <div className={styles['loggining-inputs']}>
                <Field 
                label='Логин' 
                value={loginformData.login}
                id='login'
                onChange={handleLoginFormData}
                error={errors.login}
                />
                <Field 
                isPassword={true} 
                label='Пароль' 
                value={loginformData.password}
                id='password'
                onChange={handleLoginFormData}
                error={errors.password}
                />
            </div>
            <Button className={styles['form-button']} type='submit'>Войти</Button>
        </form>
    )
}

export default LoginForm