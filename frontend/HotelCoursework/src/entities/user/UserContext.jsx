import { createContext, useEffect } from "react";
import useUser from "./useUser";

export const UserContext = createContext({})

export const UserContextProvider = (props)=>{
    const {children} = props
    const{
        user,
        setUser
    }= useUser()
    useEffect(() => {
        const savedUser = localStorage.getItem('user');
        if (savedUser) {
            try {
                const parsedUser = JSON.parse(savedUser);
                setUser(parsedUser);
            } catch (error) {
                localStorage.removeItem('user');
            }
        }
    }, []);

    const updateUser = (newUser) => {
        setUser(newUser);
        
        if (newUser) {
            localStorage.setItem('user', JSON.stringify(newUser));
        } else {
            localStorage.removeItem('user');
        }
    };

    const logout = async () => {
        try {
            await loginApi.logout();
        } catch (error) {
            console.error('Ошибка при выходе:', error);
        } finally {
            updateUser(null);
        }
    };

    return(
        <UserContext.Provider value={{ user, setUser: updateUser, logout }}>
            {children}
        </UserContext.Provider>
    )
}